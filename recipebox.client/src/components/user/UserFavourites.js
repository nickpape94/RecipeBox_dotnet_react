import React, { useEffect, useState, Fragment } from 'react';
import PropTypes from 'prop-types';
import { getFavourites } from '../../actions/favourite';
import { getUser } from '../../actions/user';
import { connect } from 'react-redux';
import Spinner from '../layout/Spinner';
import PostItem from '../posts/PostItem';
import { Link } from 'react-router-dom';
import PageNavigation from '../posts/PageNavigation';

const UserFavourites = ({
	getFavourites,
	favourites: { favourites, favouritesLoading },
	getUser,
	user: { user, loading },
	match,
	pagination
}) => {
	// const [ pageNumber, setPageNumber ] = useState(1);
	// const [ loadingPage, setLoadingPage ] = useState(false);
	// const [ searched, setSearch ] = useState(false);
	// const [ searchQuery, setSearchQuery ] = useState('');
	// const [ sortData, setSortData ] = useState('');

	useEffect(
		() => {
			getFavourites(match.params.id);
		},
		[ getFavourites, match.params.id ]
	);

	useEffect(
		() => {
			getUser(match.params.id);
		},
		[ getUser, match.params.id ]
	);

	// if (loadingPage) {
	// 	return <Spinner />;
	// }

	return (
		<Fragment>
			{favouritesLoading || loading || user === null ? (
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
						<h1 className='large text-primary m-3'>{user.username.split(' ')[0]}'s Favourites</h1>
					</div>

					<Link to={`/users/${user.id}`} className='btn'>
						<i className='fas fa-arrow-circle-left' /> Back To {user.username.split(' ')[0]}'s Profile
					</Link>
					<div className='dropdown2 m-3'>
						<select
							className='test2'
							// name='orderBy'
							type='text'
							placeholder='Select Cuisine'
							name='orderBy'
							// value={orderBy}
							required
							// onChange={(e) => onChange(e)}
						>
							<option value=''>*Sort by</option>
							<option value='newest'>Newest</option>
							<option value='oldest'>Oldest</option>
							<option value='highest rated'>Highest rated</option>
							<option value='most discussed'>Most discussed</option>
						</select>
					</div>
					{/* <PageNavigation pagination={pagination} pageNumber={pageNumber} setPageNumber={setPageNumber} /> */}

					<div className='cards'>
						{favourites.map((post) => (
							<PostItem
								key={post.postId}
								post={post}
								// postPhoto={post.postPhoto.filter((photo) => photo.isMain == true)}
							/>
						))}
					</div>
				</Fragment>
			)}
		</Fragment>
	);
};

UserFavourites.propTypes = {
	getFavourites: PropTypes.func.isRequired,
	getUser: PropTypes.func.isRequired,
	favourites: PropTypes.object.isRequired,
	user: PropTypes.object.isRequired
};

const mapStateToProps = (state) => ({
	favourites: state.favourites,
	user: state.user
});

export default connect(mapStateToProps, { getFavourites, getUser })(UserFavourites);
