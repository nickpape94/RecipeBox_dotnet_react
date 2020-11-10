import React, { Fragment, useEffect, useState, useRef } from 'react';
import { Link, useLocation } from 'react-router-dom';
import PropTypes from 'prop-types';
import { connect } from 'react-redux';
import { getPosts } from '../../actions/post';
import Spinner from '../layout/Spinner';
import PostItem from './PostItem';
import PageNavigation from './PageNavigation';

const Posts = ({ getPosts, post: { posts, loading }, auth, pagination, location }) => {
	// const [ pageNumber, setPageNumber ] = useState(pagination.currentPage !== null ? pagination.currentPage : 1);
	const [ pageNumber, setPageNumber ] = useState(
		pagination.currentPage === null || (location.state !== undefined && location.state.fromNav)
			? 1
			: pagination.currentPage
	);
	const [ loadingPage, setLoadingPage ] = useState(false);
	const [ searched, setSearch ] = useState(false);
	const [ searchQuery, setSearchQuery ] = useState('');
	const [ sortData, setSortData ] = useState({
		searchParams: '',
		orderBy: '',
		userId: ''
	});

	const { searchParams, orderBy, userId } = sortData;

	useEffect(
		() => {
			getPosts({ pageNumber, setLoadingPage, searchParams, orderBy, userId });
		},
		[ getPosts, pageNumber, orderBy ]
	);

	if (loadingPage) {
		return <Spinner />;
	}

	// Go to page 1 if a search is made
	if (searched) {
		setPageNumber(1);
		setSearch(false);
	}

	const onChange = (e) => {
		setSortData({ ...sortData, [e.target.name]: e.target.value });
	};

	const onSubmit = async (e) => {
		e.preventDefault();
		setSearch(true);
		setSearchQuery(searchParams);
		getPosts({ pageNumber, setLoadingPage, searchParams, orderBy, userId });
	};

	// console.log(history.location);

	return loading ? (
		<Spinner />
	) : (
		<Fragment>
			<div className='post__navbar'>
				<form className='search__wrapper' onSubmit={(e) => onSubmit(e)}>
					<input
						type='text'
						placeholder='Search by Cuisine, Ingredient, Author or Recipe..'
						name='searchParams'
						value={searchParams}
						onChange={(e) => onChange(e)}
					/>
					<button type='submit' className='searchbtn'>
						<i className='fas fa-search' />
					</button>
				</form>
				<div className='dropdown2'>
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
				</div>

				{/* <div className='post__dropdown'>
					<div class='dropdown'>
						<button class='dropbtn'>Sort Recipes By:</button>
						<div class='dropdown-content'>
							<Link to='!#'>Highest Rated</Link>
							<Link to='!#'>Oldest</Link>
							<Link to='!#'>Newest</Link>
							<Link to='!#'>Most Discussed</Link>
						</div>
					</div>
				</div>{' '} */}
				<div className='post__submit'>
					{auth.isAuthenticated && (
						<Link to='submit-post'>
							<div className='button'>Submit a Recipe</div>
						</Link>
					)}
					{!auth.isAuthenticated && (
						<Link to='login'>
							<div className='button'>Submit a Recipe</div>
						</Link>
					)}
				</div>
			</div>

			{searchQuery.length > 0 && <strong>{`${pagination.totalItems} results for "${searchQuery}"`}</strong>}

			<PageNavigation pagination={pagination} pageNumber={pageNumber} setPageNumber={setPageNumber} />

			<div className='cards'>
				{posts.map((post) => (
					<PostItem
						key={post.postId}
						post={post}
						// postPhoto={post.postPhoto.filter((photo) => photo.isMain == true)}
					/>
				))}
			</div>

			<PageNavigation pagination={pagination} pageNumber={pageNumber} setPageNumber={setPageNumber} />
			{/* <div className='pagination2'>
				<PageNavigation pagination={pagination} pageNumber={pageNumber} setPageNumber={setPageNumber} />
			</div> */}
		</Fragment>
	);
};

Posts.propTypes = {
	getPosts: PropTypes.func.isRequired,
	post: PropTypes.object.isRequired,
	auth: PropTypes.object.isRequired,
	pagination: PropTypes.object.isRequired
};

const mapStateToProps = (state) => ({
	post: state.post,
	auth: state.auth,
	pagination: state.pagination
});

export default connect(mapStateToProps, { getPosts })(Posts);
