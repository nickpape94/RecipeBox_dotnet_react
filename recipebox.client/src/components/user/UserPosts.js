import React, { useEffect, useState, Fragment } from 'react';
import PropTypes from 'prop-types';
import { getPosts } from '../../actions/post';
import { getUser } from '../../actions/user';
import { connect } from 'react-redux';
import Spinner from '../layout/Spinner';
import PostItem from '../posts/PostItem';
import { Link } from 'react-router-dom';
import PageNavigation from '../posts/PageNavigation';

const UserPosts = ({ getPosts, post: { posts, loading }, getUser, user: { user }, match, profilePagination }) => {
	const [ pageNumber, setPageNumber ] = useState(
		profilePagination.currentPage !== null ? profilePagination.currentPage : 1
	);
	const [ loadingPage, setLoadingPage ] = useState(false);
	const [ userLoading, setUserLoading ] = useState(false);
	const [ sortData, setSortData ] = useState({
		searchParams: '',
		orderBy: '',
		userId: match.params.id.toString()
	});

	const { searchParams, orderBy, userId } = sortData;

	useEffect(
		() => {
			getPosts({ pageNumber, setLoadingPage, searchParams, orderBy, userId });
		},
		[ getPosts, orderBy, userId, pageNumber ]
	);

	useEffect(
		() => {
			getUser(match.params.id, setUserLoading);
		},
		[ getUser, match.params.id ]
	);

	const onChange = (e) => {
		setSortData({ ...sortData, [e.target.name]: e.target.value });
	};

	if (loadingPage || userLoading) {
		return <Spinner />;
	}

	return (
		<Fragment>
			{loading ? (
				<Spinner />
			) : (
				<Fragment>
					<div>
						{/* {user.photoUrl ? (
							<img className='round-img my-1' src={user.photoUrl} alt='' />
						) : (
							<img
								className='round-img my-1'
								src='https://kansai-resilience-forum.jp/wp-content/uploads/2019/02/IAFOR-Blank-Avatar-Image-1.jpg'
								alt='no avatar'
							/>
						)} */}
						<h1 className='large text-primary m-3'>{user.username.split(' ')[0]}'s Posts</h1>
					</div>

					<Link to={`/users/${user.id}`} className='btn'>
						<i className='fas fa-arrow-circle-left' /> Back To {user.username.split(' ')[0]}'s Profile
					</Link>
					{/* <div className='dropdown2 m-3'>
						<select
							className='test2'
							name='orderBy'
							type='text'
							value={orderBy}
							required
							onChange={(e) => onChange(e)}
						>
							<option value=''>*Sort by</option>
							<option value='newest'>Newest</option>
							<option value='oldest'>Oldest</option>
							<option value='highest rated'>Highest rated</option>
							<option value='most discussed'>Most discussed</option>
						</select>
					</div> */}
					<PageNavigation
						pagination={profilePagination}
						pageNumber={pageNumber}
						setPageNumber={setPageNumber}
					/>

					<div className='cards'>
						{posts.length === 0 ? (
							<h1>{user.username.split(' ')[0]} has not submitted any posts yet</h1>
						) : (
							posts.map((post) => (
								<PostItem
									key={post.postId}
									post={post}
									postsFromProfile={true}
									// postPhoto={post.postPhoto.filter((photo) => photo.isMain == true)}
								/>
							))
						)}
					</div>

					<PageNavigation
						pagination={profilePagination}
						pageNumber={pageNumber}
						setPageNumber={setPageNumber}
					/>
				</Fragment>
			)}
		</Fragment>
	);
};

UserPosts.propTypes = {
	getPosts: PropTypes.func.isRequired,
	getUser: PropTypes.func.isRequired,
	post: PropTypes.object.isRequired,
	profilePagination: PropTypes.object.isRequired,
	user: PropTypes.object.isRequired
};

const mapStateToProps = (state) => ({
	profilePagination: state.profilePagination,
	post: state.post,
	user: state.user
});

export default connect(mapStateToProps, { getPosts, getUser })(UserPosts);
